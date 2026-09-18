import { useEffect, useState } from 'react'
import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { ChevronLeft, ChevronRight, Pencil, Plus, Search, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Modal } from '@/components/ui/modal'
import { useAuth } from '@/contexts/AuthContext'
import { useCategories } from '@/hooks/useCategories'
import {
  useCreateProduct,
  useDeleteProduct,
  useProducts,
  useUpdateProduct,
  useUpdateStock,
  type ProductInput,
} from '@/hooks/useProducts'
import type { Product } from '@/types'

const currency = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

const productSchema = z.object({
  name: z.string().min(1, 'Nome e obrigatorio').max(200, 'Maximo de 200 caracteres'),
  description: z.string().max(1000, 'Maximo de 1000 caracteres').optional(),
  price: z.coerce.number({ error: 'Informe um preco valido' }).min(0, 'Preco nao pode ser negativo'),
  categoryId: z.string().min(1, 'Selecione uma categoria'),
  stockQuantity: z.coerce
    .number({ error: 'Informe uma quantidade valida' })
    .int('Deve ser um numero inteiro')
    .min(0, 'Estoque nao pode ser negativo'),
})

type ProductFormInput = z.input<typeof productSchema>
type ProductFormOutput = z.output<typeof productSchema>

const PAGE_SIZE = 10

export function ProductsPage() {
  const { hasRole } = useAuth()
  const isAdmin = hasRole('admin')

  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [debouncedSearch, setDebouncedSearch] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [formModal, setFormModal] = useState<{ product: Product | null } | null>(null)
  const [stockModal, setStockModal] = useState<Product | null>(null)

  // Debounce simples pra nao disparar uma request a cada tecla digitada.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setDebouncedSearch(search)
      setPage(1)
    }, 400)
    return () => clearTimeout(timeout)
  }, [search])

  const { data: categories } = useCategories()
  const { data, isLoading, isError } = useProducts({
    page,
    pageSize: PAGE_SIZE,
    search: debouncedSearch || undefined,
    categoryId: categoryId || undefined,
  })

  const createProduct = useCreateProduct()
  const updateProduct = useUpdateProduct()
  const deleteProduct = useDeleteProduct()

  function handleDelete(product: Product) {
    if (!window.confirm(`Excluir o produto "${product.name}"? Essa acao nao pode ser desfeita.`)) return
    deleteProduct.mutate(product.id, {
      onError: () => window.alert('Nao foi possivel excluir o produto.'),
    })
  }

  function handleFormSubmit(input: ProductInput) {
    if (formModal?.product) {
      updateProduct.mutate(
        { id: formModal.product.id, input },
        {
          onSuccess: () => setFormModal(null),
          onError: () => window.alert('Nao foi possivel salvar o produto.'),
        },
      )
    } else {
      createProduct.mutate(input, {
        onSuccess: () => setFormModal(null),
        onError: () => window.alert('Nao foi possivel criar o produto.'),
      })
    }
  }

  const totalPages = data?.totalPages ?? 1

  return (
    <div className="flex flex-col gap-6">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
        <div>
          <h1 className="text-2xl font-semibold">Produtos</h1>
          <p className="text-sm text-muted-foreground">Gerencie o catalogo e o estoque dos produtos.</p>
        </div>
        {isAdmin && (
          <Button onClick={() => setFormModal({ product: null })}>
            <Plus className="size-4" />
            Novo produto
          </Button>
        )}
      </div>

      <Card>
        <CardContent className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <div className="relative flex-1">
            <Search className="absolute top-2.5 left-3 size-4 text-muted-foreground" />
            <Input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Buscar por nome..."
              className="pl-9"
            />
          </div>
          <select
            value={categoryId}
            onChange={(event) => {
              setCategoryId(event.target.value)
              setPage(1)
            }}
            className="h-9 rounded-md border bg-transparent px-3 text-sm shadow-xs outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 sm:w-56"
          >
            <option value="">Todas as categorias</option>
            {categories?.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          {isLoading ? (
            <p className="text-sm text-muted-foreground">Carregando produtos...</p>
          ) : isError || !data ? (
            <p className="text-sm text-destructive">Nao foi possivel carregar os produtos.</p>
          ) : data.items.length === 0 ? (
            <p className="text-sm text-muted-foreground">Nenhum produto encontrado.</p>
          ) : (
            <>
              <div className="overflow-x-auto">
                <table className="w-full text-left text-sm">
                  <thead>
                    <tr className="border-b text-muted-foreground">
                      <th className="pb-2 font-medium">Nome</th>
                      <th className="pb-2 font-medium">Categoria</th>
                      <th className="pb-2 font-medium">Preco</th>
                      <th className="pb-2 font-medium">Estoque</th>
                      {isAdmin && <th className="pb-2 font-medium">Acoes</th>}
                    </tr>
                  </thead>
                  <tbody>
                    {data.items.map((product) => (
                      <tr key={product.id} className="border-b last:border-0">
                        <td className="py-2">{product.name}</td>
                        <td className="py-2 text-muted-foreground">{product.categoryName ?? '-'}</td>
                        <td className="py-2">{currency.format(product.price)}</td>
                        <td className="py-2">
                          <button
                            type="button"
                            onClick={() => isAdmin && setStockModal(product)}
                            disabled={!isAdmin}
                            className={
                              product.isLowStock
                                ? 'font-medium text-destructive underline decoration-dotted underline-offset-4 disabled:no-underline'
                                : 'underline decoration-dotted underline-offset-4 disabled:no-underline'
                            }
                            title={isAdmin ? 'Clique para ajustar o estoque' : undefined}
                          >
                            {product.stockQuantity}
                          </button>
                        </td>
                        {isAdmin && (
                          <td className="py-2">
                            <div className="flex gap-1">
                              <button
                                type="button"
                                onClick={() => setFormModal({ product })}
                                className="rounded-md p-1.5 text-muted-foreground hover:bg-accent"
                                aria-label={`Editar ${product.name}`}
                              >
                                <Pencil className="size-4" />
                              </button>
                              <button
                                type="button"
                                onClick={() => handleDelete(product)}
                                className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-destructive"
                                aria-label={`Excluir ${product.name}`}
                              >
                                <Trash2 className="size-4" />
                              </button>
                            </div>
                          </td>
                        )}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="mt-4 flex items-center justify-between text-sm text-muted-foreground">
                <span>
                  Pagina {data.page} de {totalPages || 1} - {data.totalCount} produto(s)
                </span>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setPage((current) => Math.max(1, current - 1))}
                    disabled={page <= 1}
                  >
                    <ChevronLeft className="size-4" />
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
                    disabled={page >= totalPages}
                  >
                    <ChevronRight className="size-4" />
                  </Button>
                </div>
              </div>
            </>
          )}
        </CardContent>
      </Card>

      {formModal && (
        <ProductFormModal
          product={formModal.product}
          onClose={() => setFormModal(null)}
          onSubmit={handleFormSubmit}
          isSubmitting={createProduct.isPending || updateProduct.isPending}
        />
      )}

      {stockModal && <StockModal product={stockModal} onClose={() => setStockModal(null)} />}
    </div>
  )
}

function ProductFormModal({
  product,
  onClose,
  onSubmit,
  isSubmitting,
}: {
  product: Product | null
  onClose: () => void
  onSubmit: (input: ProductInput) => void
  isSubmitting: boolean
}) {
  const { data: categories } = useCategories()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ProductFormInput, unknown, ProductFormOutput>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: product?.name ?? '',
      description: product?.description ?? '',
      price: product?.price ?? 0,
      categoryId: product?.categoryId ?? '',
      stockQuantity: product?.stockQuantity ?? 0,
    },
  })

  return (
    <Modal open onClose={onClose} title={product ? 'Editar produto' : 'Novo produto'}>
      <form
        onSubmit={handleSubmit((values) =>
          onSubmit({ ...values, description: values.description || undefined }),
        )}
        className="flex flex-col gap-4"
      >
        <div>
          <Label htmlFor="name">Nome</Label>
          <Input id="name" className="mt-1" {...register('name')} />
          {errors.name && <p className="mt-1 text-xs text-destructive">{errors.name.message}</p>}
        </div>

        <div>
          <Label htmlFor="description">Descricao (opcional)</Label>
          <Input id="description" className="mt-1" {...register('description')} />
          {errors.description && (
            <p className="mt-1 text-xs text-destructive">{errors.description.message}</p>
          )}
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <Label htmlFor="price">Preco</Label>
            <Input id="price" type="number" step="0.01" min="0" className="mt-1" {...register('price')} />
            {errors.price && <p className="mt-1 text-xs text-destructive">{errors.price.message}</p>}
          </div>
          <div>
            <Label htmlFor="stockQuantity">Estoque</Label>
            <Input id="stockQuantity" type="number" min="0" className="mt-1" {...register('stockQuantity')} />
            {errors.stockQuantity && (
              <p className="mt-1 text-xs text-destructive">{errors.stockQuantity.message}</p>
            )}
          </div>
        </div>

        <div>
          <Label htmlFor="categoryId">Categoria</Label>
          <select
            id="categoryId"
            className="mt-1 h-9 w-full rounded-md border bg-transparent px-3 text-sm shadow-xs outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50"
            {...register('categoryId')}
          >
            <option value="">Selecione...</option>
            {categories?.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
          {errors.categoryId && (
            <p className="mt-1 text-xs text-destructive">{errors.categoryId.message}</p>
          )}
        </div>

        <div className="mt-2 flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Salvando...' : 'Salvar'}
          </Button>
        </div>
      </form>
    </Modal>
  )
}

function StockModal({ product, onClose }: { product: Product; onClose: () => void }) {
  const [quantity, setQuantity] = useState(product.stockQuantity)
  const updateStock = useUpdateStock()

  function handleSubmit() {
    updateStock.mutate(
      { id: product.id, quantity },
      {
        onSuccess: onClose,
        onError: () => window.alert('Nao foi possivel atualizar o estoque.'),
      },
    )
  }

  return (
    <Modal open onClose={onClose} title={`Ajustar estoque - ${product.name}`}>
      <div className="flex flex-col gap-4">
        <div>
          <Label htmlFor="quantity">Quantidade em estoque</Label>
          <Input
            id="quantity"
            type="number"
            min="0"
            className="mt-1"
            value={quantity}
            onChange={(event) => setQuantity(Number(event.target.value))}
          />
        </div>
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="button" onClick={handleSubmit} disabled={updateStock.isPending}>
            {updateStock.isPending ? 'Salvando...' : 'Salvar'}
          </Button>
        </div>
      </div>
    </Modal>
  )
}
